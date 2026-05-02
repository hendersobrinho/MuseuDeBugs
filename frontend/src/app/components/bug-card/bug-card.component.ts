import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BugCard } from '../../models/bug-card';

@Component({
  selector: 'app-bug-card',
  standalone: true,
  templateUrl: './bug-card.component.html'
})
export class BugCardComponent {
  @Input({ required: true }) bug!: BugCard;
  @Output() bugSelected = new EventEmitter<BugCard>();

  get statusClass(): string {
    const statusMap: Record<BugCard['status'], string> = {
      Aberto: 'status-open',
      Resolvido: 'status-done'
    };

    return statusMap[this.bug.status];
  }

  selectBug(): void {
    this.bugSelected.emit(this.bug);
  }

  selectBugWithKeyboard(event: Event): void {
    event.preventDefault();
    this.selectBug();
  }
}
