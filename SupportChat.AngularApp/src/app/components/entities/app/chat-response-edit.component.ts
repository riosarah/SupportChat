//@CustomCode
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ChatResponseBaseEditComponent }from '@app/components/entities/app/chat-response-base-edit.component';
@Component({
  standalone: true,
  selector:'app-chat-response-edit',
  imports: [ CommonModule, FormsModule, TranslateModule],
  templateUrl: './chat-response-edit.component.html',
  styleUrl: './chat-response-edit.component.css'
})
export class ChatResponseEditComponent extends ChatResponseBaseEditComponent {
}
