import { Component, EventEmitter, Output } from '@angular/core';

import { BugFilter } from '../../models/bug-filter';

@Component({
  selector: 'app-groove-strip',
  standalone: true,
  templateUrl: './groove-strip.component.html'
})
export class GrooveStripComponent {
  @Output() filterSelected = new EventEmitter<BugFilter>();

  readonly pads: BugFilter[] = [
    { code: 'A1', label: 'Todos' },
    { code: 'A2', label: 'Abertos', status: 'Aberto' },
    { code: 'B1', label: 'Resolvidos', status: 'Resolvido' },
    { code: 'B2', label: 'C#', linguagem: 'C#' },
    { code: 'C1', label: 'SQL', linguagem: 'SQL' },
    { code: 'C2', label: 'Angular', linguagem: 'Angular' },
  ];
  activeFilter = this.pads[0];

  selectFilter(filter: BugFilter): void {
    this.activeFilter = filter;
    this.filterSelected.emit(filter);
  }
}
