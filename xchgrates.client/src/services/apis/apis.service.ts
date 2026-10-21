import { Service } from "@angular/core";
import { ApiBase } from "../common/apiBase.service";
import { GetApisResponse, GetCurrenciesResponse } from "./apis.models";

@Service()
export class ApisService extends ApiBase {

  async getApis(): Promise<GetApisResponse[] | null> {
    return await this.get<GetApisResponse[]>('/api');
  }

  async getCurrencies(apiId: string): Promise<GetCurrenciesResponse | null> {
    return await this.get<GetCurrenciesResponse>(`/api/currencies?apiId=${encodeURIComponent(apiId)}`);
  }

}
