import { formatDate } from "@angular/common";
import { Service } from "@angular/core";
import { ApiBase } from "../common/apiBase.service";
import { GetExchangeRatesResponse } from "./exchangeRates.models";

@Service()
export class ExchangeRatesService extends ApiBase {

  async getExchangeRates(apiId: string, sourceCurrencyCode: string, targetCurrencyCode: string, startDate: Date | null = null, endDate: Date | null = null): Promise<GetExchangeRatesResponse | null> {
    const params = [];
    params.push(`apiId=${encodeURIComponent(apiId)}`);
    params.push(`sourceCurrencyCode=${encodeURIComponent(sourceCurrencyCode)}`);
    params.push(`targetCurrencyCode=${encodeURIComponent(targetCurrencyCode)}`);
    if (startDate !== null) {
      params.push(`startDate=${encodeURIComponent(formatDate(startDate, "yyyy-MM-dd", "en-US"))}`);
    }
    if (endDate !== null) {
      params.push(`endDate=${encodeURIComponent(formatDate(endDate, "yyyy-MM-dd", "en-US"))}`);
    }

    return await this.get<GetExchangeRatesResponse>(`/api/exchangeRates?${params.join('&')}`);
  }

}
