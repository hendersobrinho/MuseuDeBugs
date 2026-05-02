import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { BugResponse } from '../../models/bug-response';
import { CriarBugRequest } from '../../models/criar-bug-request';
import { BugService } from '../../services/bug.service';
import {
  normalizeBugLanguage,
  normalizeOptionalBugText,
  validateBugRequestForm
} from '../../utils/bug-request-form';

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
    if (this.isSubmitting) {
      return;
    }

    const request = this.buildRequest();
    const validationMessage = validateBugRequestForm(request);

    if (validationMessage) {
      this.errorMessage = validationMessage;
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
        this.successMessage = '';
        this.isSubmitting = false;
      }
    });
  }

  onFormInput(): void {
    if (this.errorMessage) {
      this.errorMessage = '';
    }

    if (this.successMessage) {
      this.successMessage = '';
    }
  }

  private buildRequest(): CriarBugRequest {
    return {
      titulo: this.form.titulo.trim(),
      linguagem: normalizeBugLanguage(this.form.linguagem),
      mensagemErro: normalizeOptionalBugText(this.form.mensagemErro),
      descricao: this.form.descricao.trim(),
      causa: normalizeOptionalBugText(this.form.causa),
      solucao: normalizeOptionalBugText(this.form.solucao)
    };
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
      return this.getApiValidationMessage(error) ?? 'Revise os campos do formulario.';
    }

    return 'Nao foi possivel registrar o bug agora.';
  }

  private getApiValidationMessage(error: HttpErrorResponse): string | null {
    const validationErrors = error.error?.errors;

    if (!validationErrors || typeof validationErrors !== 'object') {
      return null;
    }

    const messages = Object.values(validationErrors)
      .flatMap((value) => Array.isArray(value) ? value : [value])
      .filter((value): value is string => typeof value === 'string');

    return messages[0] ?? null;
  }
}
