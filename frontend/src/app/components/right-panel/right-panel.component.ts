import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';

import { BugResponse } from '../../models/bug-response';
import { BugService } from '../../services/bug.service';
import { BugCreatePanelComponent } from '../bug-create-panel/bug-create-panel.component';

interface LanguageStat {
  name: string;
  count: number;
}

@Component({
  selector: 'app-right-panel',
  standalone: true,
  imports: [BugCreatePanelComponent],
  templateUrl: './right-panel.component.html'
})
export class RightPanelComponent implements OnChanges, OnInit {
  @Input() openCreateSignal = 0;
  @Input() bugAtualizado: BugResponse | null = null;
  @Input() bugDeletadoId: number | null = null;
  @Output() bugCriado = new EventEmitter<BugResponse>();

  totalBugs = 0;
  resolvedBugs = 0;
  openBugs = 0;
  languageStats: LanguageStat[] = [];
  isLoading = true;
  hasError = false;
  private bugs: BugResponse[] = [];

  constructor(private readonly bugService: BugService) {}

  ngOnInit(): void {
    this.bugService.listar().subscribe({
      next: (bugs) => {
        this.bugs = bugs;
        this.calculateStats(this.bugs);
        this.isLoading = false;
      },
      error: () => {
        this.hasError = true;
        this.isLoading = false;
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['bugAtualizado'] && this.bugAtualizado) {
      this.upsertBug(this.bugAtualizado);
    }

    if (changes['bugDeletadoId'] && this.bugDeletadoId != null) {
      this.bugs = this.bugs.filter((bug) => bug.id !== this.bugDeletadoId);
      this.calculateStats(this.bugs);
    }
  }

  handleBugCriado(bug: BugResponse): void {
    this.upsertBug(bug);
    this.bugCriado.emit(bug);
  }

  private upsertBug(bug: BugResponse): void {
    this.bugs = [
      bug,
      ...this.bugs.filter((currentBug) => currentBug.id !== bug.id)
    ];
    this.calculateStats(this.bugs);
  }

  private calculateStats(bugs: BugResponse[]): void {
    this.totalBugs = bugs.length;
    this.resolvedBugs = bugs.filter((bug) => bug.status === 'Resolvido').length;
    this.openBugs = bugs.filter((bug) => bug.status === 'Aberto').length;
    this.languageStats = this.calculateLanguageStats(bugs);
  }

  private calculateLanguageStats(bugs: BugResponse[]): LanguageStat[] {
    const stats = new Map<string, number>();

    for (const bug of bugs) {
      const language = bug.linguagem.trim();
      stats.set(language, (stats.get(language) ?? 0) + 1);
    }

    return Array.from(stats, ([name, count]) => ({ name, count }))
      .sort((current, next) => next.count - current.count)
      .slice(0, 3);
  }
}
