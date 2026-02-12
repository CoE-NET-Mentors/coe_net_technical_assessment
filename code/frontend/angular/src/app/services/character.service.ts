import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Character {
  id: number;
  name: string;
  status: string;
  species: string;
  type: string;
  gender: string;
  originName: string;
  locationName: string;
  image: string;
  episodeCount: number;
  externalId: number;
  externalUrl: string;
  createdAt: string;
  updatedAt: string;
}

export interface CharacterResponse {
  count: number;
  data: Character[];
}

@Injectable({
  providedIn: 'root'
})
export class CharacterService {
  private apiUrl = (window as any).RUNTIME_CONFIG?.apiUrl || environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Get all characters from the API
  getAllCharacters(): Observable<CharacterResponse> {
    return this.http.get<CharacterResponse>(this.apiUrl);
  }

  // Get a specific character by ID
  getCharacterById(id: number): Observable<Character> {
    return this.http.get<Character>(`${this.apiUrl}/${id}`);
  }

  // Create a new character
  createCharacter(character: any): Observable<Character> {
    return this.http.post<Character>(this.apiUrl, character);
  }

  // Update an existing character
  updateCharacter(id: number, character: any): Observable<Character> {
    return this.http.put<Character>(`${this.apiUrl}/${id}`, character);
  }

  // Delete a character
  deleteCharacter(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
