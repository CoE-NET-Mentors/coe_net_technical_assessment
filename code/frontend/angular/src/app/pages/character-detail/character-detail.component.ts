import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CharacterService, Character } from '../../services/character.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-character-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './character-detail.component.html',
  styleUrls: ['./character-detail.component.css']
})
export class CharacterDetailComponent implements OnInit {
  character: Character | null = null;
  loading = true;
  error = '';

  constructor(private route: ActivatedRoute, private characterService: CharacterService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    const snapshotId = this.route.snapshot?.paramMap?.get('id');
    if (snapshotId) {
      this.loadCharacter(Number(snapshotId));
    }

    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      if (id) {
        this.loadCharacter(id);
      }
    });
  }

  private loadCharacter(id: number): void {
    if (!id) {
      this.error = 'Invalid character id';
      this.loading = false;
      this.cdr.markForCheck();
      return;
    }

    this.loading = true;
    this.error = '';

    this.characterService.getCharacterById(id).pipe(
      finalize(() => { this.loading = false; this.cdr.markForCheck(); })
    ).subscribe({
      next: (c) => {
        console.log('Detail character loaded:', c);
        this.character = c;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.error = 'Failed to load character';
        console.error(err);
        this.cdr.markForCheck();
      }
    });
  }
}
