//@CustomCode
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { TicketStepBaseEditComponent }from '@app/components/entities/app/ticket-step-base-edit.component';
@Component({
  standalone: true,
  selector:'app-ticket-step-edit',
  imports: [ CommonModule, FormsModule, TranslateModule],
  templateUrl: './ticket-step-edit.component.html',
  styleUrl: './ticket-step-edit.component.css'
})
export class TicketStepEditComponent extends TicketStepBaseEditComponent {
}
