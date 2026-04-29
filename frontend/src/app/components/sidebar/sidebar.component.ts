import { Component } from '@angular/core';
import { AuthPanelComponent } from '../auth-panel/auth-panel.component';

interface MenuItem {
  label: string;
  marker: string;
  active?: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [AuthPanelComponent],
  templateUrl: './sidebar.component.html'
})
export class SidebarComponent {
  readonly menuItems: MenuItem[] = [
    { label: 'Acervo de Bugs', marker: 'AC', active: true },
    { label: 'Novo Bug', marker: 'NB' },
    { label: 'Linguagens', marker: 'LG' },
    { label: 'Status', marker: 'ST' },
    { label: 'Aprendizados', marker: 'AP' },
    { label: 'Configuracoes', marker: 'CF' }
  ];
}
