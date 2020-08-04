import { Component, OnInit, Input } from '@angular/core';
import { Hero } from '../hero';
import { HEROES } from '../mock-heroes';
import { ActivatedRoute } from '@angular/router';
import { HeroService } from '../services/hero.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-hero-detail',
  templateUrl: './hero-detail.component.html',
  styleUrls: ['./hero-detail.component.css']
})
export class HeroDetailComponent implements OnInit {

  hero: Hero;

  constructor(private route: ActivatedRoute, private heroService: HeroService, private location: Location) { }

  ngOnInit() {
    this.getHero();
  }

  getHero() {
    const id = +this.route.snapshot.paramMap.get('id');
    this.heroService.getHero(id).subscribe(hero => this.hero = hero);
  }

  save():void {
    this.heroService.updateHero(this.hero).subscribe(() => this.goBack())
  }
  
  goBack(): void {
    this.location.back();
  }
}
