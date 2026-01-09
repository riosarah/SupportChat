//@CustomCode
import { IdType, IdDefault } from '@app/models/i-key-model';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IQueryParams } from '@app/models/base/i-query-params';
import { IChatMessage } from '@app-models/entities/app/i-chat-message';
import { ChatMessageBaseListComponent }from '@app/components/entities/app/chat-message-base-list.component';
import { ChatMessageEditComponent }from '@app/components/entities/app/chat-message-edit.component';
import { AuthService } from '@app-services/auth.service';
import { Role } from '@app/models/account/role';
@Component({
  standalone: true,
  selector:'app-chat-message-list',
  imports: [ CommonModule, FormsModule, TranslateModule, RouterModule ],
  templateUrl: './chat-message-list.component.html',
  styleUrl: './chat-message-list.component.css'
})
export class ChatMessageListComponent extends ChatMessageBaseListComponent {
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
    queryParams.filter = 'content.ToLower().Contains(@0)';
  }
  protected override getItemKey(item: IChatMessage): IdType {
    return item?.id || IdDefault;
  }
  override get pageTitle(): string {
    return 'ChatMessages';
  }
  override getEditComponent() {
    return ChatMessageEditComponent;
  }
}
