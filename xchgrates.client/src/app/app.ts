import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html'
})
export class App implements OnInit {
  public forecasts = signal<WeatherForecast[]>([]);

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getForecasts();
  }

  getForecasts() {
    this.http.get<WeatherForecast[]>('/weatherforecast').subscribe({
      next: (result) => {
        this.forecasts.set(result);
        console.log("get forecast result", this.forecasts);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  onClickGenerate() {
    this.forecasts.set([
      { date: '2026-10-10', temperatureC: 25, temperatureF: 26, summary: 'somewhat' }
    ]);
  }

  protected readonly title = signal('xchgrates.client');
}
