import { Component, OnInit } from '@angular/core';
import { Hero } from '../hero';
import { HeroService } from '../services/hero.service';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-heroes',
  templateUrl: './heroes.component.html',
  styleUrls: ['./heroes.component.css']
})
export class HeroesComponent implements OnInit {

  heroes: Hero[];
  selectedHero: Hero;

  constructor(private heroService: HeroService, private messageService: MessageService) { }

  ngOnInit() {
    this.getHeroes();
  }

  onSelect(hero: Hero): void {
    this.messageService.add(`${this.constructor.name}.onSelect(${JSON.stringify(hero)})`);
    this.selectedHero = hero;
  }
  
  getHeroes(): void {
    this.heroService.getHeroes().subscribe(this.setHeroes);
  }

  private setHeroes = (heroes: Hero[]) => {
      this.heroes = heroes;
    };
}
