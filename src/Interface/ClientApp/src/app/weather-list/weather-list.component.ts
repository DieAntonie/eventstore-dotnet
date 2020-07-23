import { Component } from '@angular/core';
import { WeatherService } from '../weather-service/weather.service';

@Component({
  selector: 'app-weather-list',
  templateUrl: './weather-list.component.html'
})
export class WeatherListComponent {
  public forecasts: WeatherForecast[];

  constructor(private weatherService: WeatherService) {
    this.weatherService.getWeather().subscribe(weather => this.forecasts = weather);
  }

  onNotify() {
    window.alert("GROOT KAKas");
  }
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
