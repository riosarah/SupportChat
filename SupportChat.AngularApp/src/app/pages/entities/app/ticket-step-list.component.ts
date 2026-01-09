//@CustomCode
import { IdType, IdDefault } from '@app/models/i-key-model';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IQueryParams } from '@app/models/base/i-query-params';
import { ITicketStep } from '@app-models/entities/app/i-ticket-step';
import { TicketStepBaseListComponent }from '@app/components/entities/app/ticket-step-base-list.component';
import { TicketStepEditComponent }from '@app/components/entities/app/ticket-step-edit.component';
import { AuthService } from '@app-services/auth.service';
import { Role } from '@app/models/account/role';
@Component({
  standalone: true,
  selector:'app-ticket-step-list',
  imports: [ CommonModule, FormsModule, TranslateModule, RouterModule ],
  templateUrl: './ticket-step-list.component.html',
  styleUrl: './ticket-step-list.component.css'
})
export class TicketStepListComponent extends TicketStepBaseListComponent {
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
    queryParams.filter = 'description.ToLower().Contains(@0)';
  }
  protected override getItemKey(item: ITicketStep): IdType {
    return item?.id || IdDefault;
  }
  override get pageTitle(): string {
    return 'TicketSteps';
  }
  override getEditComponent() {
    return TicketStepEditComponent;
  }
}
