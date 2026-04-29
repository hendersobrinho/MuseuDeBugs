import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';

import { BugCard } from '../../models/bug-card';
import { BugResponse } from '../../models/bug-response';
import { BugService } from '../../services/bug.service';
import { BugCardComponent } from '../bug-card/bug-card.component';

@Component({
  selector: 'app-bug-grid',
  standalone: true,
  imports: [BugCardComponent],
  templateUrl: './bug-grid.component.html'
})
export class BugGridComponent implements OnInit, OnChanges {
  @Input() bugBuscado: BugResponse | null = null;
  @Input() bugCriado: BugResponse | null = null;

  bugs: BugCard[] = [];
  private todosOsBugs: BugCard[] = [];

  isLoading = true;
  errorMessage = '';

  constructor(private readonly bugService: BugService) {}

  ngOnInit(): void {
    this.carregarBugs();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['bugCriado'] && this.bugCriado) {
      this.adicionarBugCriado(this.bugCriado);
    }

    if (changes['bugBuscado']) {
      this.aplicarBuscaPorId();
    }
  }

  private carregarBugs(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.bugService.listar().subscribe({
      next: (bugsDaApi) => {
        this.todosOsBugs = bugsDaApi.map((bug) => this.converterParaCard(bug));
        this.aplicarBuscaPorId();
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Nao foi possivel carregar os bugs.';
        this.isLoading = false;
      }
    });
  }

  private aplicarBuscaPorId(): void {
    if (this.bugBuscado) {
      this.bugs = [this.converterParaCard(this.bugBuscado)];
      return;
    }

    this.bugs = this.todosOsBugs;
  }

  private adicionarBugCriado(bug: BugResponse): void {
    const card = this.converterParaCard(bug);
    this.todosOsBugs = [
      card,
      ...this.todosOsBugs.filter((currentBug) => currentBug.id !== card.id)
    ];

    if (!this.bugBuscado) {
      this.bugs = this.todosOsBugs;
    }
  }

  private converterParaCard(bug: BugResponse): BugCard {
    return {
      id: bug.id,
      title: bug.titulo,
      language: bug.linguagem,
      status: bug.status as BugCard['status'],
      errorMessage: bug.mensagemErro || 'Sem mensagem de erro registrada',
      cause: bug.causa || bug.descricao,
      updatedAt: bug.dataAtualizacao || bug.dataCriacao,
      signal: bug.status === 'Resolvido' ? 'learned' : 'info'
    };
  }
}
