import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HeroesComponent } from './heroes/heroes.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { WeatherListComponent } from './weather-list/weather-list.component';
import { WeatherDetailsComponent } from './weather-details/weather-details.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HeroDetailComponent } from './hero-detail/hero-detail.component';

const routes: Routes = [
    { path: '', component: HomeComponent, pathMatch: 'full' },
    { path: 'counter', component: CounterComponent },
    { path: 'weather-list', component: WeatherListComponent },
    { path: 'weather-details/:summary', component: WeatherDetailsComponent },
    { path: 'heroes', component: HeroesComponent },
    { path: 'hero-detail/:id', component: HeroDetailComponent },
    { path: 'dashboard', component: DashboardComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
