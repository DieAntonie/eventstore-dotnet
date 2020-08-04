import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of, ObservableInput } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HEROES } from '../mock-heroes';
import { Hero } from '../hero';
import { MessageService } from './message.service';

@Injectable({
  providedIn: 'root',
})
export class HeroService {
  private readonly heroesUrl = 'hero';  // URL to web api
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(private http: HttpClient, private messageService: MessageService, @Inject('BASE_URL') private baseUrl: string) { }

  getHeroes(): Observable<Hero[]> {
    this.log(`getHeroes()`);
    return this.http.get<Hero[]>(`${this.baseUrl + this.heroesUrl}`).pipe(
      tap((tapVal: Hero[]) => this.log(`getHeroes() => ${tapVal.map(t => JSON.stringify(t))}`)),
      catchError(this.handleError<Hero[]>('getHeroes', []))
    );
  }
  
  getHero(id: number): Observable<Hero> {
    this.log(`getHero(${id})`);
    return this.http.get<Hero>(`${this.baseUrl + this.heroesUrl}/${id}`).pipe(
      tap((tapVal: Hero) => this.log(`getHero(${id}) => ${JSON.stringify(tapVal)}`)),
      catchError(this.handleError<Hero>('getHero'))
    );
  }

  updateHero(hero: Hero): Observable<any> {
    this.log(`updateHero(${JSON.stringify(hero)})`);
    return this.http.put(`${this.baseUrl + this.heroesUrl}/${hero.id}`, hero, this.httpOptions).pipe(
      tap(tapVal => this.log(`updateHero(${JSON.stringify(hero)}) => ${JSON.stringify(tapVal)}`)),
      catchError(this.handleError<any>('updateHero'))
    );
  }

  addHero(hero: Hero): Observable<Hero> {
    return this.http.post<Hero>(`${this.baseUrl + this.heroesUrl}`, hero, this.httpOptions).pipe(
      tap((tapVal: Hero) => this.log(`addHero(${JSON.stringify(hero)}) => ${JSON.stringify(tapVal)}`)),
      catchError(this.handleError<Hero>('addHero'))
    );
  }

  deleteHero(hero: Hero | number): Observable<Hero> {
    const id = typeof hero === 'number' ? hero : hero.id;  
    return this.http.delete<Hero>(`${this.baseUrl + this.heroesUrl}/${id}`, this.httpOptions).pipe(
      tap((tapVal: Hero) => this.log(`deleteHero(${JSON.stringify(hero)}) => ${JSON.stringify(tapVal)}`)),
      catchError(this.handleError<Hero>('deleteHero'))
    );
  }

  searchHeroes(term: string): Observable<Hero[]> {
    if (!term.trim()) {
      // if not search term, return empty hero array.
      return of([]);
    }
    return this.http.get<Hero[]>(`${this.baseUrl + this.heroesUrl}/?name=${term}`).pipe(
      tap((tapVal: Hero[]) => tapVal.length ?
         this.log(`searchHeroes(${term}) => ${JSON.stringify(tapVal)}`) :
         this.log(`searchHeroes(${term}) => []`)),
      catchError(this.handleError<Hero[]>('searchHeroes', []))
    );
  }

  handleError<T>(operation: string, result?: T) {
    return (error: any, caught: Observable<T>): Observable<T> => {
      console.error(error);
      this.log(`${operation} failed: ${error.message}`);
      return of(result as T);
    };
  }

  private log(message: string): void {
    this.messageService.add(`${this.constructor.name}: ${message}`);
  }
}
