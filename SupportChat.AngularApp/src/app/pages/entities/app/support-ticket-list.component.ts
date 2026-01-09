//@CustomCode
import { IdType, IdDefault } from '@app/models/i-key-model';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IQueryParams } from '@app/models/base/i-query-params';
import { ISupportTicket } from '@app-models/entities/app/i-support-ticket';
import { SupportTicketBaseListComponent }from '@app/components/entities/app/support-ticket-base-list.component';
import { SupportTicketEditComponent }from '@app/components/entities/app/support-ticket-edit.component';
import { AuthService } from '@app-services/auth.service';
import { Role } from '@app/models/account/role';
@Component({
  standalone: true,
  selector:'app-support-ticket-list',
  imports: [ CommonModule, FormsModule, TranslateModule, RouterModule ],
  templateUrl: './support-ticket-list.component.html',
  styleUrl: './support-ticket-list.component.css'
})
export class SupportTicketListComponent extends SupportTicketBaseListComponent {
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
    queryParams.filter = 'problemDescription.ToLower().Contains(@0) OR systemInfo.ToLower().Contains(@0) OR recommendation.ToLower().Contains(@0)';
  }
  protected override getItemKey(item: ISupportTicket): IdType {
    return item?.id || IdDefault;
  }
  override get pageTitle(): string {
    return 'SupportTickets';
  }
  override getEditComponent() {
    return SupportTicketEditComponent;
  }
}
