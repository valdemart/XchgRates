import { formatDate } from "@angular/common";
import { Service, Provider, inject, linkedSignal } from "@angular/core";
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, NativeDateAdapter } from "@angular/material/core";
import { SettingsService } from "./settings/settings.service";

@Service()
export class FormatableDateAdapter extends NativeDateAdapter {
  private settingsService: SettingsService = inject(SettingsService);

  public dateFormat = linkedSignal({
    source: () => this.settingsService.settings(),
    computation: (settings) => settings?.dateFormat ?? 'yyyy-MM-dd'
  });

  formatDate(date: Date): string {
    return this.format(date, 'input');
  }
  
  override format(date: Date, displayFormat: Object): string {
    if (displayFormat === 'input') { //for inputs
      return formatDate(date, this.dateFormat(), "en-US");
    }
    
    return super.format(date, displayFormat); //for everything else
  }

  override getFirstDayOfWeek(): number {
    return 1; //Monday is first
  }
}

export const CUSTOM_DATE_FORMATS = {
  parse: {
    dateInput: 'input',
  },
  display: {
    dateInput: 'input',
    monthYearLabel: { year: 'numeric', month: 'short' },
    dateA11yLabel: { year: 'numeric', month: 'long', day: 'numeric' },
    monthYearA11yLabel: { year: 'numeric', month: 'long' },
  },
};



export function provideFormatableDateProvider(): Provider[] {
  return [
    { provide: DateAdapter, useClass: FormatableDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: CUSTOM_DATE_FORMATS }
  ]
}
