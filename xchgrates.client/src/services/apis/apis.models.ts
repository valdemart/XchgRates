//Get APIs response
export interface GetApisResponse {
  apiId: string;
  displayName: string;
  nearestToday: Date;
}

//Get currencies response
export interface GetCurrenciesResponse {
  sourceCurrencies: CurrencyData[];
  targetCurrencies: CurrencyData[];
}

export interface CurrencyData {
  code: string;
  name: string;
}
