//@CodeCopy
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@environment/environment';
import { ILogon } from '@app-models/account/i-logon';
import { IAuthenticatedUser } from '@app-models/account/i-authenticated-user';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private BASE_URL = environment.API_BASE_URL + '/accounts';

  constructor(private httpClient: HttpClient) { }

  public async login(logonData: ILogon): Promise<IAuthenticatedUser> {
    console.log('📡 [ACCOUNT-SERVICE] login() aufgerufen');
    console.log('📡 [ACCOUNT-SERVICE] BASE_URL:', this.BASE_URL);
    console.log('📡 [ACCOUNT-SERVICE] Login URL:', this.BASE_URL + '/login');
    console.log('📡 [ACCOUNT-SERVICE] LogonData:', logonData);
    
    try {
      console.log('📡 [ACCOUNT-SERVICE] Sende HTTP POST Request...');
      const response = await firstValueFrom(
        this.httpClient.post<IAuthenticatedUser>(
          this.BASE_URL + '/login',
          logonData
        )
      );
      console.log('📡 [ACCOUNT-SERVICE] HTTP Response erhalten:', response);
      return response;
    } catch (error) {
      console.error('📡 [ACCOUNT-SERVICE] HTTP Request FEHLER:', error);
      throw error;
    }
  }

  public logout(sessionToken: string): Promise<any> {
    return firstValueFrom(
      this.httpClient.delete<any>(`${this.BASE_URL}/${sessionToken}`)
    );
  }

  public isSessionAlive(sessionToken: string): Promise<boolean> {
    const body = { sessionToken };

    return firstValueFrom(
      this.httpClient.post<boolean>(`${this.BASE_URL}/issessionalive`, body)
    );
  }

  public async requestPassword(email: string): Promise<any> {
    return firstValueFrom(
      this.httpClient.post<IAuthenticatedUser>(
        this.BASE_URL + '/requestPassword',
        {
          email: email,
        }
      )
    );
  }

  public async setPassword(
    email: string,
    code: string,
    password: string
  ): Promise<any> {
    return firstValueFrom(
      this.httpClient.post<IAuthenticatedUser>(this.BASE_URL + '/setPassword', {
        email: email,
        code: code,
        password: password,
      })
    );
  }

  public async changePassword(oldPassword: string, newPassword: string): Promise<any> {
    return firstValueFrom(
      this.httpClient.post<IAuthenticatedUser>(
        this.BASE_URL + '/changePassword',
        {
          oldPassword: oldPassword,
          newPassword: newPassword,
        }
      )
    );
  }
}
