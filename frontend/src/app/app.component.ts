import { Component } from '@angular/core';
import { BugResponse } from './models/bug-response';
import { BugGridComponent } from './components/bug-grid/bug-grid.component';
import { GrooveStripComponent } from './components/groove-strip/groove-strip.component';
import { RightPanelComponent } from './components/right-panel/right-panel.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { TopbarComponent } from './components/topbar/topbar.component';
import { BugFilter } from './models/bug-filter';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    SidebarComponent,
    TopbarComponent,
    GrooveStripComponent,
    BugGridComponent,
    RightPanelComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  bugBuscado: BugResponse | null = null;
  bugCriado: BugResponse | null = null;
  bugAlterado: BugResponse | null = null;
  bugDeletadoId: number | null = null;
  activeFilter: BugFilter | null = null;
  termoBusca = '';
  createPanelSignal = 0;

  handleRegistrarBugSolicitado(): void {
    this.createPanelSignal += 1;
  }

  handleBugCriado(bug: BugResponse): void {
    this.bugBuscado = null;
    this.bugCriado = bug;
    this.bugAlterado = bug;
    this.bugDeletadoId = null;
  }

  handleBugAlterado(bug: BugResponse): void {
    if (this.bugBuscado?.id === bug.id) {
      this.bugBuscado = bug;
    }

    this.bugAlterado = bug;
    this.bugDeletadoId = null;
  }

  handleBugDeletado(id: number): void {
    if (this.bugBuscado?.id === id) {
      this.bugBuscado = null;
    }

    this.bugDeletadoId = id;
    this.bugAlterado = null;
  }

  handleTermoBuscaAlterado(termo: string): void {
    this.termoBusca = termo;
    this.bugBuscado = null;
  }
}
