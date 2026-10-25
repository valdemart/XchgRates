import { Service, signal } from "@angular/core";
import { ApiBase } from "../common/apiBase.service";
import { GetSettingsResponse } from "./settings.models";

@Service()
export class SettingsService extends ApiBase {
  private readonly _settings = signal<GetSettingsResponse | null>(null);
  readonly settings = this._settings.asReadonly();

  async loadSettings(): Promise<GetSettingsResponse | null> {
    const newSettings = await this.get<GetSettingsResponse>('/api/settings') ?? this._settings();

    this._settings.set(newSettings);

    return newSettings;
  }
}
