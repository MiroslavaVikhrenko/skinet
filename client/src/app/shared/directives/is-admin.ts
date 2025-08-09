import { Directive, effect, inject, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../../core/services/account.service';

@Directive({
  selector: '[appIsAdmin]' // *appIsAdmin
})
export class IsAdmin {
  private accountService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  constructor() { 
    effect(() => {
      if (this.accountService.isAdmin()) {
      this.viewContainerRef.createEmbeddedView(this.templateRef); // view container to display template, 
      // use directive on element where this is checked
    } else {
      this.viewContainerRef.clear();
    }
    })
  }

}
