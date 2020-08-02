import { NgModule } from '@angular/core';

// Imports: The set of NgModules whose exported declarables are available to templates in this module.
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Declarations: The set of components, directives, and pipes that belong to this module.
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { WeatherListComponent } from './weather-list/weather-list.component';
import { WeatherAlertComponent } from './weather-alert/weather-alert.component';
import { WeatherDetailsComponent } from './weather-details/weather-details.component';
import { HeroesComponent } from './heroes/heroes.component';
import { HeroDetailComponent } from './hero-detail/hero-detail.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    WeatherListComponent,
    WeatherAlertComponent,
    WeatherDetailsComponent,
    HeroesComponent,
    HeroDetailComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'weather-list', component: WeatherListComponent },
      { path: 'weather-details/:summary', component: WeatherDetailsComponent },
      { path: 'heroes', component: HeroesComponent },
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
