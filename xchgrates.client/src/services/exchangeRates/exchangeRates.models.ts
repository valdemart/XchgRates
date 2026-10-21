import { CurrencyData } from "../apis/apis.models";

//Get exchange rates response
export interface GetExchangeRatesResponse {
  source: CurrencyData;
  target: CurrencyData;
  minRate: number;
  maxRate: number;
  avgRate: number;
  rates: GetExchangeRatesResponseRate[];
}

export interface GetExchangeRatesResponseRate {
  date: Date;
  rate: number;
}
