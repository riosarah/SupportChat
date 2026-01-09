import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChatbotComponent } from './chatbot.component';
import { TranslateModule } from '@ngx-translate/core';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

describe('ChatbotComponent', () => {
  let component: ChatbotComponent;
  let fixture: ComponentFixture<ChatbotComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ChatbotComponent,
        TranslateModule.forRoot(),
        HttpClientTestingModule,
        RouterTestingModule
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChatbotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not be active initially', () => {
    expect(component.isChatActive).toBeFalsy();
  });

  it('should start chat when startNewChat is called', () => {
    component.startNewChat();
    expect(component.isChatActive).toBeTruthy();
    expect(component.messages.length).toBeGreaterThan(0);
  });
});
