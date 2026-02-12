import { TestBed } from '@angular/core/testing';
import { CharacterDetailComponent } from './character-detail.component';
import { CharacterService } from '../../services/character.service';
import { of } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

describe('CharacterDetailComponent', () => {
  beforeEach(async () => {
    const mockService = {
      getCharacterById: () => of({
        id: 1,
        name: 'Rick',
        status: 'Alive',
        species: 'Human',
        type: 'Humanoid',
        gender: 'Male',
        originName: 'Earth',
        locationName: 'Citadel of Ricks',
        image: 'https://example.com/rick.png',
        episodeCount: 51,
        externalId: 1,
        externalUrl: 'https://rickandmorty.com/api/character/1',
        createdAt: '2021-01-01',
        updatedAt: '2021-01-01'
      })
    };

    const mockActivatedRoute = {
      snapshot: { paramMap: { get: () => '1' } },
      paramMap: of({ get: (key: string) => key === 'id' ? '1' : null })
    };

    await TestBed.configureTestingModule({
      imports: [CharacterDetailComponent],
      providers: [
        { provide: CharacterService, useValue: mockService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();
  });

  it('creates the component', () => {
    const fixture = TestBed.createComponent(CharacterDetailComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('loads and displays character details', (done) => {
    const fixture = TestBed.createComponent(CharacterDetailComponent);
    fixture.detectChanges();
    const cmp = fixture.componentInstance;
    setTimeout(() => {
      expect(cmp.character?.name).toBe('Rick');
      expect(cmp.character?.species).toBe('Human');
      expect(cmp.loading).toBe(false);
      done();
    }, 100);
  });

  it('displays image when available', (done) => {
    const fixture = TestBed.createComponent(CharacterDetailComponent);
    fixture.detectChanges();
    const cmp = fixture.componentInstance;
    setTimeout(() => {
      expect(cmp.character?.image).toBe('https://example.com/rick.png');
      done();
    }, 100);
  });

  it('shows loading state initially', () => {
    const fixture = TestBed.createComponent(CharacterDetailComponent);
    const cmp = fixture.componentInstance;
    expect(cmp.loading).toBe(true);
  });

  it('clears loading after data loads', (done) => {
    const fixture = TestBed.createComponent(CharacterDetailComponent);
    fixture.detectChanges();
    const cmp = fixture.componentInstance;
    setTimeout(() => {
      expect(cmp.loading).toBe(false);
      done();
    }, 100);
  });
});
