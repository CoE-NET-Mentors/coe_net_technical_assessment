import { TestBed } from '@angular/core/testing';
import { CharacterEditComponent } from './character-edit.component';
import { CharacterService } from '../../services/character.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';

describe('CharacterEditComponent', () => {
  beforeEach(async () => {
    const mockService = {
      getCharacterById: () => of({ id: 2, name: 'Morty', status: 'Alive', species: 'Human', type: '', gender: 'Male', originName: '', locationName: '', image: '', episodeCount: 0, externalId: 0, externalUrl: '', createdAt: '', updatedAt: '' }),
      updateCharacter: () => of({})
    };

    const mockActivatedRoute = {
      snapshot: { paramMap: { get: () => '2' } },
      paramMap: of({ get: (key: string) => key === 'id' ? '2' : null })
    };

    await TestBed.configureTestingModule({
      imports: [CharacterEditComponent],
      providers: [
        { provide: CharacterService, useValue: mockService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: Router, useValue: { navigate: () => {} } }
      ]
    }).compileComponents();
  });

  it('loads character into form', () => {
    const fixture = TestBed.createComponent(CharacterEditComponent);
    fixture.detectChanges();
    const cmp = fixture.componentInstance;
    expect(cmp.form.name).toBe('Morty');
  });
});
