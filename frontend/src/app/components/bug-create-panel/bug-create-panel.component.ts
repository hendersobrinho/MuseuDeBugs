import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { BugResponse } from '../../models/bug-response';
import { CriarBugRequest } from '../../models/criar-bug-request';
import { BugService } from '../../services/bug.service';

@Component({
  selector: 'app-bug-create-panel',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './bug-create-panel.component.html'
})
export class BugCreatePanelComponent implements OnChanges {
  @Input() openSignal = 0;
  @Output() bugCriado = new EventEmitter<BugResponse>();

  form: CriarBugRequest = this.createEmptyForm();
  isOpen = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(private readonly bugService: BugService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['openSignal'] && !changes['openSignal'].firstChange) {
      this.isOpen = true;
      this.errorMessage = '';
      this.successMessage = '';
    }
  }

  toggleForm(): void {
    this.isOpen = !this.isOpen;
    this.errorMessage = '';
    this.successMessage = '';
  }

  salvar(): void {
    const request = this.buildRequest();

    if (!this.isValid(request)) {
      this.errorMessage = 'Preencha titulo, linguagem e descricao.';
      this.successMessage = '';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.bugService.criar(request).subscribe({
      next: (bug) => {
        this.bugCriado.emit(bug);
        this.form = this.createEmptyForm();
        this.successMessage = `Bug #${bug.id} registrado.`;
        this.isSubmitting = false;
        this.isOpen = false;
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.getErrorMessage(error);
        this.isSubmitting = false;
      }
    });
  }

  private buildRequest(): CriarBugRequest {
    return {
      titulo: this.form.titulo.trim(),
      linguagem: this.form.linguagem.trim(),
      mensagemErro: this.normalizeOptionalText(this.form.mensagemErro),
      descricao: this.form.descricao.trim(),
      causa: this.normalizeOptionalText(this.form.causa),
      solucao: this.normalizeOptionalText(this.form.solucao)
    };
  }

  private isValid(request: CriarBugRequest): boolean {
    return request.titulo.length >= 3 &&
      request.linguagem.length >= 1 &&
      request.descricao.length >= 10;
  }

  private normalizeOptionalText(value: string | null | undefined): string | null {
    const normalized = value?.trim() ?? '';
    return normalized.length > 0 ? normalized : null;
  }

  private createEmptyForm(): CriarBugRequest {
    return {
      titulo: '',
      linguagem: '',
      mensagemErro: '',
      descricao: '',
      causa: '',
      solucao: ''
    };
  }

  private getErrorMessage(error: HttpErrorResponse): string {
    if (error.status === 401 || error.status === 403) {
      return 'Faca login como admin para registrar bugs.';
    }

    if (error.status === 400) {
      return 'Revise os campos do formulario.';
    }

    return 'Nao foi possivel registrar o bug agora.';
  }
}
