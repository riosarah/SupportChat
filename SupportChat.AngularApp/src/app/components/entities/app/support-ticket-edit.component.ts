//@CustomCode
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { SupportTicketBaseEditComponent }from '@app/components/entities/app/support-ticket-base-edit.component';
@Component({
  standalone: true,
  selector:'app-support-ticket-edit',
  imports: [ CommonModule, FormsModule, TranslateModule],
  templateUrl: './support-ticket-edit.component.html',
  styleUrl: './support-ticket-edit.component.css'
})
export class SupportTicketEditComponent extends SupportTicketBaseEditComponent {
}
