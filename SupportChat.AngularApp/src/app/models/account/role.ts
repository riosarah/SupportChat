import { IdType } from '@app-models/i-key-model';

export enum Role {
  SysAdmin = 'SysAdmin',
  AppAdmin = 'AppAdmin',
  AppUser = 'AppUser',
}

export interface IRoleInfo {
  id: IdType;
  designation: string;
  description: string;
}
