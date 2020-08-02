import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HEROES } from '../mock-heroes';
import { Hero } from '../hero';
import { MessageService } from './message.service';

@Injectable({
  providedIn: 'root',
})
export class HeroService {

  constructor(private messageService: MessageService) { }

  getHeroes(): Observable<Hero[]> {
    this.messageService.add(`${this.constructor.name}.getHeroes()`);
    return of(HEROES);
  }
  
  getHero(id: number): Observable<Hero> {
    this.messageService.add(`${this.constructor.name}.getHero(${id})`);
    return of(HEROES.find(hero => hero.id === id));
  }
}
