import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BugResponse } from '../../models/bug-response';
import { BugService } from '../../services/bug.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './topbar.component.html'
})
export class TopbarComponent {
  @Output() buscaPorIdConcluida = new EventEmitter<BugResponse | null>();
  @Output() registrarBugSolicitado = new EventEmitter<void>();

  bugId = '';
  erroBusca = '';
  carregandoBusca = false;

  constructor(private readonly bugService: BugService) {}

  aoAlterarBugId(value: string): void {
    this.bugId = `${value ?? ''}`;

    if (this.bugId.trim() === '') {
      this.erroBusca = '';
      this.carregandoBusca = false;
      this.buscaPorIdConcluida.emit(null);
    }
  }

  buscarBugPorId(): void {
    const value = this.bugId.trim();

    if (value === '') {
      this.erroBusca = '';
      this.buscaPorIdConcluida.emit(null);
      return;
    }

    const id = Number(value);

    if (!Number.isInteger(id) || id <= 0) {
      this.erroBusca = 'Digite um id valido.';
      this.buscaPorIdConcluida.emit(null);
      return;
    }

    this.carregandoBusca = true;
    this.erroBusca = '';

    this.bugService.buscarPorId(id).subscribe({
      next: (bug) => {
        this.buscaPorIdConcluida.emit(bug);
        this.carregandoBusca = false;
      },
      error: () => {
        this.erroBusca = 'Bug nao encontrado.';
        this.buscaPorIdConcluida.emit(null);
        this.carregandoBusca = false;
      }
    });
  }

  solicitarRegistroBug(): void {
    this.registrarBugSolicitado.emit();
  }
}
