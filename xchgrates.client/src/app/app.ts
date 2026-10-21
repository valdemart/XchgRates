import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, inject, OnInit, resource, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { GetApisResponse, GetCurrenciesResponse } from '../services/apis/apis.models';
import { ApisService } from '../services/apis/apis.service';
import { GetExchangeRatesResponse } from '../services/exchangeRates/exchangeRates.models';
import { ExchangeRatesService } from '../services/exchangeRates/exchangeRates.service';

@Component({
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    MatButtonModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  providers: [
    provideNativeDateAdapter()
  ]
})
export class App implements OnInit {
  private apisService: ApisService = inject(ApisService);
  private exchangeRatesService: ExchangeRatesService = inject(ExchangeRatesService);

  public receivedApis = signal<GetApisResponse[]>([]);
  public selectedApi = signal<string | null>(null);

  public currencies = resource({
    params: () => ({ newSelectedApi: this.selectedApi() }),
    loader: ({params}) => this.onSelectedApiChange(params.newSelectedApi),
  });
  public selectedSourceCurrency = signal<string | null>(null);
  public selectedTargetCurrency = signal<string | null>(null);

  public selectedStartDate = signal<Date | null>(null);
  public selectedEndDate = signal<Date | null>(null);

  public ratesData = signal<GetExchangeRatesResponse | null>(null);

  ngOnInit() {
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
      this.selectedApi.set(newApisSet[0].apiId);
    }

  }

  async onSelectedApiChange(newSelectedApi: string | null): Promise<GetCurrenciesResponse | null> {
    if (newSelectedApi === null) {
      return null;
    }

    return await this.apisService.getCurrencies(newSelectedApi);
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
        selectedApi,
        sourceCurrency,
        targetCurrency,
        startDate,
        endDate
      );
    }

    this.ratesData.set(newRates);
  }
}
