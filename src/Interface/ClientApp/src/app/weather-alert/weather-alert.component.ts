import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-weather-alert',
  templateUrl: './weather-alert.component.html'
})
export class WeatherAlertComponent {
  @Input() weather;
  @Output() notify = new EventEmitter();

  constructor() {}
}
