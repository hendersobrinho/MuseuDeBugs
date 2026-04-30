import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { BugFilter } from '../../models/bug-filter';

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
  @Input() activeFilter: BugFilter | null = null;
  @Input() termoBusca = '';


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

    if (changes['bugBuscado'] || changes['activeFilter'] || changes['termoBusca']) {
      this.aplicarFiltros();
    }
  }

  private carregarBugs(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.bugService.listar().subscribe({
      next: (bugsDaApi) => {
        this.todosOsBugs = bugsDaApi.map((bug) => this.converterParaCard(bug));
        this.aplicarFiltros();
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Nao foi possivel carregar os bugs.';
        this.isLoading = false;
      }
    });
  }

  private aplicarFiltros(): void {
    if (this.bugBuscado) {
      this.bugs = [this.converterParaCard(this.bugBuscado)];
      return;
    }

    let bugsFiltrados = this.todosOsBugs;

    if (this.activeFilter?.status) {
      bugsFiltrados = bugsFiltrados.filter((bug) =>
        bug.status === this.activeFilter?.status
      );
    }

    if (this.activeFilter?.linguagem) {
      bugsFiltrados = bugsFiltrados.filter((bug) =>
        bug.language === this.activeFilter?.linguagem
      );
    }

    const termoNormalizado = this.termoBusca.trim().toLowerCase();

    if (termoNormalizado) {
      bugsFiltrados = bugsFiltrados.filter((bug) =>
        this.normalizarTexto([
          bug.id,
          bug.title,
          bug.language,
          bug.status,
          bug.errorMessage,
          bug.cause
        ].join(' ')).includes(this.normalizarTexto(termoNormalizado))
      );
    }

    this.bugs = bugsFiltrados;
  }

  private normalizarTexto(value: string): string {
    return value
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '');
  }

  private adicionarBugCriado(bug: BugResponse): void {
    const card = this.converterParaCard(bug);
    this.todosOsBugs = [
      card,
      ...this.todosOsBugs.filter((currentBug) => currentBug.id !== card.id)
    ];

    this.aplicarFiltros();
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
