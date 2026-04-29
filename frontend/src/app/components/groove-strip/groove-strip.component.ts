import { Component } from '@angular/core';

@Component({
  selector: 'app-groove-strip',
  standalone: true,
  templateUrl: './groove-strip.component.html'
})
export class GrooveStripComponent {
  readonly pads = ['A1', 'A2', 'B1', 'B2', 'C1', 'C2', 'D1', 'D2'];
}
