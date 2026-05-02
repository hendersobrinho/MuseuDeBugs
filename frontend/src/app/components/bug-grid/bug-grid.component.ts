import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BugFilter } from '../../models/bug-filter';

import { AtualizarBugRequest } from '../../models/atualizar-bug-response';
import { BugCard } from '../../models/bug-card';
import { BugResponse } from '../../models/bug-response';
import { BugService } from '../../services/bug.service';
import {
  normalizeBugLanguage,
  normalizeOptionalBugText,
  validateBugRequestForm
} from '../../utils/bug-request-form';
import { BugCardComponent } from '../bug-card/bug-card.component';

@Component({
  selector: 'app-bug-grid',
  standalone: true,
  imports: [BugCardComponent, FormsModule],
  templateUrl: './bug-grid.component.html'
})
export class BugGridComponent implements OnInit, OnChanges {
  @Input() bugBuscado: BugResponse | null = null;
  @Input() bugCriado: BugResponse | null = null;
  @Input() activeFilter: BugFilter | null = null;
  @Input() termoBusca = '';
  @Output() bugAlterado = new EventEmitter<BugResponse>();
  @Output() bugDeletado = new EventEmitter<number>();


  bugs: BugCard[] = [];
  private todosOsBugs: BugCard[] = [];

  isLoading = true;
  errorMessage = '';
  selectedBug: BugCard | null = null;
  editForm: AtualizarBugRequest = this.createEmptyEditForm();
  resolveMessage = '';
  detailMessageType: 'success' | 'error' = 'success';
  isResolving = false;
  isEditing = false;
  isSaving = false;
  isDeleting = false;

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
          bug.description,
          bug.cause,
          bug.solution
        ].join(' ')).includes(this.normalizarTexto(termoNormalizado))
      );
    }

    this.bugs = bugsFiltrados;
  }

  selectBug(bug: BugCard): void {
    this.selectedBug = bug;
    this.editForm = this.createEditForm(bug);
    this.resolveMessage = '';
    this.detailMessageType = 'success';
    this.isEditing = false;
  }

  closeDetails(): void {
    this.selectedBug = null;
    this.resolveMessage = '';
    this.isResolving = false;
    this.isEditing = false;
    this.isSaving = false;
    this.isDeleting = false;
  }

  resolveSelectedBug(): void {
    if (!this.selectedBug || this.selectedBug.status === 'Resolvido') {
      return;
    }

    this.isResolving = true;
    this.resolveMessage = '';

    this.bugService.resolver(this.selectedBug.id).subscribe({
      next: (bugResolvido) => {
        this.substituirBug(bugResolvido);
        this.bugAlterado.emit(bugResolvido);
        this.resolveMessage = 'Bug marcado como resolvido.';
        this.detailMessageType = 'success';
        this.isResolving = false;
      },
      error: () => {
        this.resolveMessage = 'Nao foi possivel resolver. Entre como admin e tente de novo.';
        this.detailMessageType = 'error';
        this.isResolving = false;
      }
    });
  }

  startEdit(): void {
    if (!this.selectedBug) {
      return;
    }

    this.editForm = this.createEditForm(this.selectedBug);
    this.resolveMessage = '';
    this.isEditing = true;
  }

  cancelEdit(): void {
    if (this.selectedBug) {
      this.editForm = this.createEditForm(this.selectedBug);
    }

    this.resolveMessage = '';
    this.isEditing = false;
  }

  updateSelectedBug(): void {
    if (!this.selectedBug || this.isSaving) {
      return;
    }

    const request = this.buildUpdateRequest();
    const validationMessage = validateBugRequestForm(request);

    if (validationMessage) {
      this.resolveMessage = validationMessage;
      this.detailMessageType = 'error';
      return;
    }

    this.isSaving = true;
    this.resolveMessage = '';

    this.bugService.atualizar(this.selectedBug.id, request).subscribe({
      next: (bugAtualizado) => {
        this.substituirBug(bugAtualizado);
        this.bugAlterado.emit(bugAtualizado);
        this.resolveMessage = 'Bug atualizado.';
        this.detailMessageType = 'success';
        this.isEditing = false;
        this.isSaving = false;
      },
      error: () => {
        this.resolveMessage = 'Nao foi possivel atualizar. Entre como admin e tente de novo.';
        this.detailMessageType = 'error';
        this.isSaving = false;
      }
    });
  }

  onEditFormInput(): void {
    if (this.resolveMessage && this.detailMessageType === 'error') {
      this.resolveMessage = '';
    }
  }

  deleteSelectedBug(): void {
    if (!this.selectedBug) {
      return;
    }

    const id = this.selectedBug.id;

    if (!window.confirm(`Deletar o bug #${id}?`)) {
      return;
    }

    this.isDeleting = true;
    this.resolveMessage = '';

    this.bugService.deletar(id).subscribe({
      next: () => {
        this.todosOsBugs = this.todosOsBugs.filter((bug) => bug.id !== id);

        if (this.bugBuscado?.id === id) {
          this.bugBuscado = null;
        }

        this.aplicarFiltros();
        this.bugDeletado.emit(id);
        this.closeDetails();
      },
      error: () => {
        this.resolveMessage = 'Nao foi possivel deletar. Entre como admin e tente de novo.';
        this.detailMessageType = 'error';
        this.isDeleting = false;
      }
    });
  }

  private substituirBug(bug: BugResponse): void {
    const card = this.converterParaCard(bug);

    this.todosOsBugs = this.todosOsBugs.map((currentBug) =>
      currentBug.id === card.id ? card : currentBug
    );

    if (this.bugBuscado?.id === bug.id) {
      this.bugBuscado = bug;
    }

    this.aplicarFiltros();
    this.selectedBug = card;
    this.editForm = this.createEditForm(card);
  }

  private createEditForm(bug: BugCard): AtualizarBugRequest {
    return {
      titulo: bug.title,
      linguagem: bug.language,
      mensagemErro: bug.errorMessage,
      descricao: bug.description,
      causa: bug.cause,
      solucao: bug.solution
    };
  }

  private createEmptyEditForm(): AtualizarBugRequest {
    return {
      titulo: '',
      linguagem: '',
      mensagemErro: '',
      descricao: '',
      causa: '',
      solucao: ''
    };
  }

  private buildUpdateRequest(): AtualizarBugRequest {
    return {
      titulo: this.editForm.titulo.trim(),
      linguagem: normalizeBugLanguage(this.editForm.linguagem),
      mensagemErro: normalizeOptionalBugText(this.editForm.mensagemErro),
      descricao: this.editForm.descricao.trim(),
      causa: normalizeOptionalBugText(this.editForm.causa),
      solucao: normalizeOptionalBugText(this.editForm.solucao)
    };
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
      errorMessage: bug.mensagemErro ?? '',
      description: bug.descricao,
      cause: bug.causa ?? '',
      solution: bug.solucao ?? '',
      createdAt: bug.dataCriacao,
      updatedAt: bug.dataAtualizacao || bug.dataCriacao,
      signal: bug.status === 'Resolvido' ? 'learned' : 'info'
    };
  }
}
