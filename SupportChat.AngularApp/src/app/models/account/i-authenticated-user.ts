//@CodeCopy
import { IdType } from '@app-models/i-key-model';
import { IRoleInfo } from '@app-models/account/role';

export interface IAuthenticatedUser {
  id: IdType;
  identityId: IdType;
  sessionToken: string;
  loginTime: string;
  logoutTime: string | null;
  name: string;
  email: string;
  optionalInfo: string;
  roles: IRoleInfo[];
  organizationId: number;
  organizationIds: number[];
}
