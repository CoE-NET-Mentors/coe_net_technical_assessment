import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map, shareReplay, tap } from 'rxjs/operators';
import { CharacterService, Character } from '../../services/character.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  characters$!: Observable<Character[]>;
  loading = true;
  error = '';
  successMessage = '';
  showCreateForm = false;
  showUpdateForm = false;
  selectedCharacterId: number | null = null;

  // Form data
  newCharacter = {
    name: '',
    status: 'Alive',
    species: '',
    gender: 'Male',
    type: '',
    originName: '',
    locationName: '',
    image: '',
    externalId: 0,
    externalUrl: ''
  };

  updateCharacter = {
    name: '',
    status: 'Alive',
    species: '',
    gender: 'Male',
    type: '',
    originName: '',
    locationName: '',
    image: '',
    externalId: 0,
    externalUrl: ''
  };

  constructor(private characterService: CharacterService, private router: Router) {}

  ngOnInit(): void {
    this.loadCharacters();
  }

  private createCharactersStream() {
    this.loading = true;
    this.error = '';
    return this.characterService.getAllCharacters().pipe(
      map(response => response.data || []),
      tap(() => (this.loading = false)),
      catchError(err => {
        console.error('Error loading characters:', err);
        this.error = 'Failed to load characters. Make sure the API is running.';
        this.loading = false;
        return of([] as Character[]);
      }),
      shareReplay(1)
    );
  }

  loadCharacters(): void {
    this.characters$ = this.createCharactersStream();
  }

  refreshCharacters(): void {
    // re-create stream to force a fresh HTTP request
    this.loadCharacters();
  }

  // CRUD Operations
  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
    if (!this.showCreateForm) {
      this.resetCreateForm();
    }
  }

  resetCreateForm(): void {
    this.newCharacter = {
      name: '',
      status: 'Alive',
      species: '',
      gender: 'Male',
      type: '',
      originName: '',
      locationName: '',
      image: '',
      externalId: 0,
      externalUrl: ''
    };
  }

  createNewCharacter(): void {
    if (!this.newCharacter.name || !this.newCharacter.species) {
      alert('Please fill in at least Name and Species');
      return;
    }

    this.characterService.createCharacter(this.newCharacter).subscribe({
      next: (response) => {
        this.successMessage = `Character "${response.name}" created successfully!`;
        setTimeout(() => (this.successMessage = ''), 3000);
        this.resetCreateForm();
        this.showCreateForm = false;
        this.loadCharacters();
      },
      error: (err) => {
        console.error('Error creating character:', err);
        alert('Failed to create character: ' + (err.error?.message || err.message));
      }
    });
  }

  toggleUpdateForm(character: Character): void {
    // navigate to the edit page instead of inline editing
    this.router.navigate([`/characters/${character.id}/edit`]);
  }

  openDetailInNewTab(character: Character): void {
    const baseUrl = window.location.href.split('#')[0];
    const url = `${baseUrl}#/characters/${character.id}`;
    window.open(url, '_blank');
  }

  saveUpdatedCharacter(): void {
    if (!this.selectedCharacterId) return;

    if (!this.updateCharacter.name || !this.updateCharacter.species) {
      alert('Please fill in at least Name and Species');
      return;
    }

    this.characterService.updateCharacter(this.selectedCharacterId, this.updateCharacter).subscribe({
      next: (response) => {
        this.successMessage = `Character "${response.name}" updated successfully!`;
        setTimeout(() => (this.successMessage = ''), 3000);
        this.showUpdateForm = false;
        this.selectedCharacterId = null;
        this.loadCharacters();
      },
      error: (err) => {
        console.error('Error updating character:', err);
        alert('Failed to update character: ' + (err.error?.message || err.message));
      }
    });
  }

  deleteCharacter(id: number, name: string): void {
    if (confirm(`Are you sure you want to delete "${name}"?`)) {
      this.characterService.deleteCharacter(id).subscribe({
        next: () => {
          this.successMessage = `Character "${name}" deleted successfully!`;
          setTimeout(() => (this.successMessage = ''), 3000);
          this.loadCharacters();
        },
        error: (err) => {
          console.error('Error deleting character:', err);
          alert('Failed to delete character: ' + (err.error?.message || err.message));
        }
      });
    }
  }

  cancelUpdate(): void {
    this.showUpdateForm = false;
    this.selectedCharacterId = null;
  }
}
