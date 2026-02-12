import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CharacterService, Character } from '../../services/character.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-character-edit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './character-edit.component.html',
  styleUrls: ['./character-edit.component.css']
})
export class CharacterEditComponent implements OnInit {
  character: Character | null = null;
  loading = true;
  error = '';

  form = {
    name: '', status: 'Alive', species: '', gender: 'Male', type: '', originName: '', locationName: '', image: '', externalUrl: ''
  };

  constructor(private route: ActivatedRoute, private router: Router, private characterService: CharacterService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    // Use snapshot first (immediate), then subscribe to paramMap (handles route changes, lazy-loaded)
    const snapshotId = this.route.snapshot?.paramMap?.get('id');
    if (snapshotId) {
      this.loadCharacter(Number(snapshotId));
    }

    // Also subscribe in case route params change or for lazy-loaded scenarios
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      if (id) {
        this.loadCharacter(id);
      }
    });
  }

  private loadCharacter(id: number): void {
    if (!id) { this.error = 'Invalid id'; this.loading = false; return; }

    this.loading = true;
    this.error = '';

    this.characterService.getCharacterById(id).pipe(
      finalize(() => { this.loading = false; this.cdr.markForCheck(); })
    ).subscribe({
      next: (c) => {
        console.log('Character loaded:', c);
        this.character = c;
        this.form = {
          name: (c as any).name || '',
          status: (c as any).status || 'Alive',
          species: (c as any).species || '',
          gender: (c as any).gender || 'Male',
          type: (c as any).type || '',
          originName: (c as any).originName || '',
          locationName: (c as any).locationName || '',
          image: (c as any).image || '',
          externalUrl: (c as any).externalUrl || ''
        } as any;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load character:', err);
        this.error = 'Failed to load character';
        this.cdr.markForCheck();
      }
    });
  }

  save(): void {
    if (!this.character) return;
    if (!this.form.name || !this.form.species) { alert('Please fill in at least Name and Species'); return; }

    this.characterService.updateCharacter(this.character.id!, this.form as any).subscribe({
      next: () => { this.router.navigate(['/home']); },
      error: (err) => { console.error(err); alert('Failed to update character'); }
    });
  }

  cancel(): void { this.router.navigate(['/home']); }
}
