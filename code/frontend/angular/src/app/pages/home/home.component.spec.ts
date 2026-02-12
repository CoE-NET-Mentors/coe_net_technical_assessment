
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { HomeComponent } from './home.component';
import { CharacterService, Character } from '../../services/character.service';

describe('HomeComponent', () => {
  let fixture: ComponentFixture<HomeComponent>;
  let component: HomeComponent;
  let characterService: jasmine.SpyObj<CharacterService>;

  const mockCharacters: Character[] = [
    {
      id: 1,
      name: 'Rick Sanchez',
      status: 'Alive',
      species: 'Human',
      gender: 'Male',
      type: '',
      originName: 'Earth (C-137)',
      locationName: 'Earth (Replacement Dimension)',
      image: 'https://example.com/rick.png',
      episodeCount: 41,
      externalId: 1,
      externalUrl: 'https://example.com/1',
      createdAt: '2026-02-09T16:34:42.000Z',
      updatedAt: '2026-02-09T16:34:42.000Z'
    },
    {
      id: 2,
      name: 'Morty Smith',
      status: 'Alive',
      species: 'Human',
      gender: 'Male',
      type: '',
      originName: 'Earth (C-137)',
      locationName: 'Earth (Replacement Dimension)',
      image: 'https://example.com/morty.png',
      episodeCount: 41,
      externalId: 2,
      externalUrl: 'https://example.com/2',
      createdAt: '2026-02-09T16:34:42.000Z',
      updatedAt: '2026-02-09T16:34:42.000Z'
    }
  ];

  const newCharacterData = {
    name: 'Summer Smith',
    status: 'Alive',
    species: 'Human',
    gender: 'Female',
    type: '',
    originName: 'Earth (C-137)',
    locationName: 'Earth (Replacement Dimension)',
    image: 'https://example.com/summer.png',
    externalId: 3,
    externalUrl: 'https://example.com/3'
  };

  const newCharacterResponse: Character = {
    id: 3,
    ...newCharacterData,
    episodeCount: 0,
    createdAt: '2026-02-11T00:00:00.000Z',
    updatedAt: '2026-02-11T00:00:00.000Z'
  };

  beforeEach(async () => {
    const characterServiceSpy = jasmine.createSpyObj('CharacterService', [
      'getAllCharacters',
      'getCharacterById',
      'createCharacter',
      'updateCharacter',
      'deleteCharacter'
    ]);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [
        { provide: CharacterService, useValue: characterServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    characterService = TestBed.inject(CharacterService) as jasmine.SpyObj<CharacterService>;

    characterService.getAllCharacters.and.returnValue(of({ count: 2, data: mockCharacters }));
    fixture.detectChanges();
  });

  describe('Component Initialization', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('should load characters on init', (done) => {
      component.characters$.subscribe((characters) => {
        expect(characters.length).toBe(2);
        expect(characters[0].name).toBe('Rick Sanchez');
        done();
      });
    });

    it('should set loading to false after characters load', (done) => {
      component.characters$.subscribe(() => {
        expect(component.loading).toBe(false);
        done();
      });
    });

    it('should initialize form properties correctly', () => {
      expect(component.showCreateForm).toBe(false);
      expect(component.showUpdateForm).toBe(false);
      expect(component.selectedCharacterId).toBeNull();
      expect(component.successMessage).toBe('');
    });
  });

  describe('Character Display (READ)', () => {
    it('should render character cards when service returns data', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const cards = compiled.querySelectorAll('.character-card');
      expect(cards.length).toBe(2);
      expect(cards[0].textContent).toContain('Rick Sanchez');
      expect(cards[1].textContent).toContain('Morty Smith');
    });

    it('should display correct character details on cards', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const firstCard = compiled.querySelector('.character-card');
      expect(firstCard?.textContent).toContain('Alive');
      expect(firstCard?.textContent).toContain('Human');
      expect(firstCard?.textContent).toContain('Male');
    });

    it('should display no characters message when list is empty', (done) => {
      characterService.getAllCharacters.and.returnValue(of({ count: 0, data: [] }));
      component.loadCharacters();
      component.characters$.subscribe(() => {
        fixture.detectChanges();
        const compiled = fixture.nativeElement as HTMLElement;
        const noCharactersMsg = compiled.querySelector('.no-characters');
        expect(noCharactersMsg?.textContent).toContain('No characters found');
        done();
      });
    });

    it('should handle service errors gracefully', (done) => {
      characterService.getAllCharacters.and.returnValue(
        throwError(() => new Error('API Error'))
      );
      component.loadCharacters();
      component.characters$.subscribe(() => {
        expect(component.error).toContain('Failed to load characters');
        expect(component.loading).toBe(false);
        done();
      });
    });
  });

  describe('Refresh Functionality', () => {
    it('should refresh characters when refresh button is clicked', () => {
      spyOn(component, 'loadCharacters');
      component.refreshCharacters();
      expect(component.loadCharacters).toHaveBeenCalled();
    });

    it('should call getAllCharacters again on refresh', () => {
      component.refreshCharacters();
      expect(characterService.getAllCharacters).toHaveBeenCalledTimes(2); // once on init, once on refresh
    });
  });

  describe('Create Character (CREATE)', () => {
    beforeEach(() => {
      component.showCreateForm = false;
    });

    it('should toggle create form visibility', () => {
      expect(component.showCreateForm).toBe(false);
      component.toggleCreateForm();
      expect(component.showCreateForm).toBe(true);
      component.toggleCreateForm();
      expect(component.showCreateForm).toBe(false);
    });

    it('should reset form when closing create form', () => {
      component.newCharacter.name = 'Test';
      component.toggleCreateForm();
      expect(component.showCreateForm).toBe(true);
      component.toggleCreateForm();
      expect(component.newCharacter.name).toBe('');
    });

    it('should validate required fields before creating', () => {
      spyOn(window, 'alert');
      component.newCharacter.name = '';
      component.newCharacter.species = '';
      component.createNewCharacter();
      expect(window.alert).toHaveBeenCalledWith('Please fill in at least Name and Species');
    });

    it('should create character successfully', (done) => {
      characterService.createCharacter.and.returnValue(of(newCharacterResponse));
      component.newCharacter = { ...newCharacterData };
      component.createNewCharacter();
      setTimeout(() => {
        expect(characterService.createCharacter).toHaveBeenCalledWith(newCharacterData);
        expect(component.successMessage).toContain('Summer Smith');
        expect(component.showCreateForm).toBe(false);
        done();
      }, 100);
    });

    it('should reload characters after successful creation', (done) => {
      characterService.createCharacter.and.returnValue(of(newCharacterResponse));
      component.newCharacter = { ...newCharacterData };
      component.createNewCharacter();
      setTimeout(() => {
        expect(characterService.getAllCharacters).toHaveBeenCalled();
        done();
      }, 100);
    });

    it('should handle create character error', (done) => {
      spyOn(window, 'alert');
      const errorResponse = new Error('Server error');
      characterService.createCharacter.and.returnValue(throwError(() => errorResponse));
      component.newCharacter = { ...newCharacterData };
      component.createNewCharacter();
      setTimeout(() => {
        expect(window.alert).toHaveBeenCalledWith(
          jasmine.stringContaining('Failed to create character')
        );
        done();
      }, 100);
    });

    it('should clear success message after timeout', (done) => {
      characterService.createCharacter.and.returnValue(of(newCharacterResponse));
      component.newCharacter = { ...newCharacterData };
      component.createNewCharacter();
      expect(component.successMessage).toBeTruthy();
      setTimeout(() => {
        expect(component.successMessage).toBe('');
        done();
      }, 3100);
    });
  });

  describe('Update Character (UPDATE)', () => {
    beforeEach(() => {
      component.showUpdateForm = false;
      component.selectedCharacterId = null;
    });

    it('navigates to edit page when edit button clicked', () => {
      const character = mockCharacters[0];
      component.toggleUpdateForm(character);
      const router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
      expect(router.navigate).toHaveBeenCalledWith([`/characters/${character.id}/edit`]);
    });

    it('should validate required fields before updating', () => {
      spyOn(window, 'alert');
      component.selectedCharacterId = 1;
      component.updateCharacter.name = '';
      component.updateCharacter.species = '';
      component.saveUpdatedCharacter();
      expect(window.alert).toHaveBeenCalledWith('Please fill in at least Name and Species');
    });

    it('should update character successfully', (done) => {
      const updatedCharacter = { ...mockCharacters[0], name: 'Rick Updated' };
      characterService.updateCharacter.and.returnValue(of(updatedCharacter));
      component.selectedCharacterId = 1;
      component.updateCharacter = { ...mockCharacters[0], name: 'Rick Updated' };
      component.saveUpdatedCharacter();
      setTimeout(() => {
        expect(characterService.updateCharacter).toHaveBeenCalledWith(1, jasmine.any(Object));
        expect(component.successMessage).toContain('Rick Updated');
        expect(component.showUpdateForm).toBe(false);
        done();
      }, 100);
    });

    it('should reload characters after successful update', (done) => {
      const updatedCharacter = { ...mockCharacters[0], name: 'Rick Updated' };
      characterService.updateCharacter.and.returnValue(of(updatedCharacter));
      component.selectedCharacterId = 1;
      component.updateCharacter = updatedCharacter;
      component.saveUpdatedCharacter();
      setTimeout(() => {
        expect(characterService.getAllCharacters).toHaveBeenCalled();
        done();
      }, 100);
    });

    it('should handle update character error', (done) => {
      spyOn(window, 'alert');
      const errorResponse = new Error('Update failed');
      characterService.updateCharacter.and.returnValue(throwError(() => errorResponse));
      component.selectedCharacterId = 1;
      component.updateCharacter = mockCharacters[0];
      component.saveUpdatedCharacter();
      setTimeout(() => {
        expect(window.alert).toHaveBeenCalledWith(
          jasmine.stringContaining('Failed to update character')
        );
        done();
      }, 100);
    });

    it('should cancel update form correctly', () => {
      component.showUpdateForm = true;
      component.selectedCharacterId = 1;
      component.cancelUpdate();
      expect(component.showUpdateForm).toBe(false);
      expect(component.selectedCharacterId).toBeNull();
    });
  });

  describe('Delete Character (DELETE)', () => {
    it('should show confirmation before deleting', () => {
      spyOn(window, 'confirm').and.returnValue(false);
      component.deleteCharacter(1, 'Rick Sanchez');
      expect(window.confirm).toHaveBeenCalledWith('Are you sure you want to delete "Rick Sanchez"?');
      expect(characterService.deleteCharacter).not.toHaveBeenCalled();
    });

    it('should delete character when confirmed', (done) => {
      spyOn(window, 'confirm').and.returnValue(true);
      characterService.deleteCharacter.and.returnValue(of(void 0));
      component.deleteCharacter(1, 'Rick Sanchez');
      setTimeout(() => {
        expect(characterService.deleteCharacter).toHaveBeenCalledWith(1);
        expect(component.successMessage).toContain('Rick Sanchez');
        done();
      }, 100);
    });

    it('should reload characters after successful deletion', (done) => {
      spyOn(window, 'confirm').and.returnValue(true);
      characterService.deleteCharacter.and.returnValue(of(void 0));
      component.deleteCharacter(1, 'Rick Sanchez');
      setTimeout(() => {
        expect(characterService.getAllCharacters).toHaveBeenCalled();
        done();
      }, 100);
    });

    it('should handle delete character error', (done) => {
      spyOn(window, 'confirm').and.returnValue(true);
      spyOn(window, 'alert');
      const errorResponse = new Error('Delete failed');
      characterService.deleteCharacter.and.returnValue(throwError(() => errorResponse));
      component.deleteCharacter(1, 'Rick Sanchez');
      setTimeout(() => {
        expect(window.alert).toHaveBeenCalledWith(
          jasmine.stringContaining('Failed to delete character')
        );
        done();
      }, 100);
    });

    it('should show success message and clear after timeout', (done) => {
      spyOn(window, 'confirm').and.returnValue(true);
      characterService.deleteCharacter.and.returnValue(of(void 0));
      component.deleteCharacter(1, 'Rick Sanchez');
      expect(component.successMessage).toBeTruthy();
      setTimeout(() => {
        expect(component.successMessage).toBe('');
        done();
      }, 3100);
    });
  });

  describe('Form State Management', () => {
    it('should not show edit buttons when no character is being edited', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const updateForms = compiled.querySelectorAll('.inline-update-form');
      expect(updateForms.length).toBe(0);
    });

    it('should reset create form data correctly', () => {
      component.newCharacter.name = 'Test';
      component.newCharacter.species = 'TestSpecies';
      component.resetCreateForm();
      expect(component.newCharacter.name).toBe('');
      expect(component.newCharacter.species).toBe('');
      expect(component.newCharacter.status).toBe('Alive');
    });

    it('should maintain form state when opening create form', () => {
      component.newCharacter.name = 'Test';
      component.showCreateForm = false;
      component.toggleCreateForm();
      expect(component.showCreateForm).toBe(true);
      expect(component.newCharacter.name).toBe('Test');
    });
  });

  describe('UI Button Interactions', () => {
    it('should render create button', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const createBtn = compiled.querySelector('.create-btn');
      expect(createBtn).toBeTruthy();
      expect(createBtn?.textContent).toContain('Create New Character');
    });

    it('should render edit and delete buttons on character cards', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const editButtons = compiled.querySelectorAll('.edit-btn');
      const deleteButtons = compiled.querySelectorAll('.delete-btn');
      expect(editButtons.length).toBe(2);
      expect(deleteButtons.length).toBe(2);
    });

    it('should render refresh button', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const refreshBtn = compiled.querySelector('.refresh-btn');
      expect(refreshBtn).toBeTruthy();
      expect(refreshBtn?.textContent).toContain('Refresh');
    });
  });

  describe('Edge Cases', () => {
    it('should handle null selectedCharacterId in saveUpdatedCharacter', () => {
      component.selectedCharacterId = null;
      component.saveUpdatedCharacter();
      expect(characterService.updateCharacter).not.toHaveBeenCalled();
    });

    it('should handle multiple rapid character creations', (done) => {
      characterService.createCharacter.and.returnValue(of(newCharacterResponse));
      component.newCharacter = { ...newCharacterData };
      component.createNewCharacter();
      // Reset form for second creation
      component.newCharacter = { ...newCharacterData, name: 'Summer Smith 2' };
      component.createNewCharacter();
      setTimeout(() => {
        expect(characterService.createCharacter).toHaveBeenCalledTimes(2);
        done();
      }, 200);
    });

    it('should handle special characters in names', (done) => {
      const specialCharacterData = { ...newCharacterData, name: "O'Brien's & Friends" };
      const specialResponse = { ...newCharacterResponse, name: "O'Brien's & Friends" };
      characterService.createCharacter.and.returnValue(of(specialResponse));
      component.newCharacter = specialCharacterData;
      component.createNewCharacter();
      setTimeout(() => {
        expect(component.successMessage).toContain("O'Brien's & Friends");
        done();
      }, 100);
    });

    it('should navigate to edit page when attempting to update a character', () => {
      const character = mockCharacters[0];
      component.toggleUpdateForm(character);
      const router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
      expect(router.navigate).toHaveBeenCalledWith([`/characters/${character.id}/edit`]);
    });
  });
});
