//@CustomCode
import { IdType, IdDefault } from '@app/models/i-key-model';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IQueryParams } from '@app/models/base/i-query-params';
import { IChatResponse } from '@app-models/entities/app/i-chat-response';
import { ChatResponseBaseListComponent }from '@app/components/entities/app/chat-response-base-list.component';
import { ChatResponseEditComponent }from '@app/components/entities/app/chat-response-edit.component';
import { AuthService } from '@app-services/auth.service';
import { Role } from '@app/models/account/role';
@Component({
  standalone: true,
  selector:'app-chat-response-list',
  imports: [ CommonModule, FormsModule, TranslateModule, RouterModule ],
  templateUrl: './chat-response-list.component.html',
  styleUrl: './chat-response-list.component.css'
})
export class ChatResponseListComponent extends ChatResponseBaseListComponent {
  constructor(
              private authService: AuthService
             )
  {
    super();
  }
  override ngOnInit(): void {
    super.ngOnInit();
    this.reloadData();
  }
  override prepareQueryParams(queryParams: IQueryParams): void {
    super.prepareQueryParams(queryParams);
    queryParams.filter = 'responseContent.ToLower().Contains(@0) OR modelUsed.ToLower().Contains(@0) OR errorMessage.ToLower().Contains(@0) OR sessionId.ToLower().Contains(@0) OR userFeedback.ToLower().Contains(@0)';
  }
  protected override getItemKey(item: IChatResponse): IdType {
    return item?.id || IdDefault;
  }
  override get pageTitle(): string {
    return 'ChatResponses';
  }
  override getEditComponent() {
    return ChatResponseEditComponent;
  }
}
