import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, OnInit, resource, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { GetApisResponse, GetCurrenciesResponse } from '../services/apis/apis.models';
import { ApisService } from '../services/apis/apis.service';
import { GetExchangeRatesResponse } from '../services/exchangeRates/exchangeRates.models';
import { ExchangeRatesService } from '../services/exchangeRates/exchangeRates.service';
import { SettingsService } from '../services/settings/settings.service';
import { FormatableDateAdapter, provideFormatableDateProvider } from '../services/formatableDateAdapter.service';
import { RedrawOnChangeDirective } from '../directives/redrawOnChange';

@Component({
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
  imports: [
    DecimalPipe,
    FormsModule,
    MatButtonModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatSelectModule,
    RedrawOnChangeDirective
  ],
  providers: [
    provideFormatableDateProvider()
  ]
})
export class App implements OnInit {
  private apisService: ApisService = inject(ApisService);
  private exchangeRatesService: ExchangeRatesService = inject(ExchangeRatesService);
  private settingsService: SettingsService = inject(SettingsService);
  private dateProvider: FormatableDateAdapter = inject(FormatableDateAdapter);

  public dateFormat = computed<string>(() => this.dateProvider.dateFormat());

  public receivedApis = signal<GetApisResponse[]>([]);
  public selectedApi = signal<GetApisResponse | null>(null);

  public currencies = resource({
    params: () => ({ newSelectedApi: this.selectedApi() }),
    loader: ({params}) => this.onSelectedApiChange(params.newSelectedApi),
  });
  public selectedSourceCurrency = signal<string | null>(null);
  public selectedTargetCurrency = signal<string | null>(null);

  public equalCurrencies = computed(() => {
    const src = this.selectedSourceCurrency();
    const tgt = this.selectedTargetCurrency();

    return !!src && !!tgt && src === tgt;
  });

  public selectedStartDate = signal<Date | null>(new Date());
  public selectedEndDate = signal<Date | null>(new Date());

  public canGetRates = computed(() => {
    const src = this.selectedSourceCurrency();
    const tgt = this.selectedTargetCurrency();

    return !!src && !!tgt && !this.equalCurrencies();
  });

  public ratesData = signal<GetExchangeRatesResponse | null>(null);

  ngOnInit() {
    this.settingsService.loadSettings();
    this.loadApis();
  }

  async loadApis() {
    const newApisSet = await this.apisService.getApis() ?? [];
    const currentSelection = this.selectedApi();

    this.receivedApis.set(newApisSet);

    if (newApisSet.length === 0 && currentSelection !== null) {
      this.selectedApi.set(null);
    }
    if (newApisSet.length > 0 && currentSelection === null) {
      this.selectedApi.set(newApisSet[0]);
    }

  }

  async onSelectedApiChange(newSelectedApi: GetApisResponse | null): Promise<GetCurrenciesResponse | null> {
    if (newSelectedApi === null) {
      return null;
    }

    if (newSelectedApi && (!this.dateFilter(this.selectedStartDate()) || !this.dateFilter(this.selectedEndDate()))) {
      this.selectedStartDate.set(newSelectedApi.nearestToday);
      this.selectedEndDate.set(newSelectedApi.nearestToday);
    }

    return await this.apisService.getCurrencies(newSelectedApi.apiId);
  }

  async loadCurrencies() {
    this.currencies.set(await this.onSelectedApiChange(this.selectedApi()));
  }

  async onShowRates() {
    let newRates = null;

    const selectedApi = this.selectedApi();
    const sourceCurrency = this.selectedSourceCurrency();
    const targetCurrency = this.selectedTargetCurrency();
    const startDate = this.selectedStartDate();
    const endDate = this.selectedEndDate();

    //validate
    if (selectedApi && sourceCurrency && targetCurrency) {
      newRates = await this.exchangeRatesService.getExchangeRates(
        selectedApi.apiId,
        sourceCurrency,
        targetCurrency,
        startDate,
        endDate
      );
    }

    this.ratesData.set(newRates);
  }

  dateFilter(d: Date | null): boolean {
    const today = new Date();
    d = d ?? today;

    return d <= today && 1 <= d.getDay() && d.getDay() <= 5;
  }
}
