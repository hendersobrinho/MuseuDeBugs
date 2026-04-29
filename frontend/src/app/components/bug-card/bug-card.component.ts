import { Component, Input } from '@angular/core';
import { BugCard } from '../../models/bug-card';

@Component({
  selector: 'app-bug-card',
  standalone: true,
  templateUrl: './bug-card.component.html'
})
export class BugCardComponent {
  @Input({ required: true }) bug!: BugCard;

  get statusClass(): string {
    const statusMap: Record<BugCard['status'], string> = {
      Aberto: 'status-open',
      Resolvido: 'status-done'
    };

    return statusMap[this.bug.status];
  }
}
