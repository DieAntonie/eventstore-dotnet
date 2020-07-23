import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {
  public forecasts: Observable<WeatherForecast[]>;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  getWeather() {
    if (!this.forecasts)
      this.forecasts = this.http.get<WeatherForecast[]>(this.baseUrl + 'weatherforecast');
    return this.forecasts;
  }

}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}