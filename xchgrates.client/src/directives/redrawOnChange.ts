import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[redrawOnChange]',
  standalone: true
})
export class RedrawOnChangeDirective {
  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}

  @Input() set redrawOnChange(value: any) {
    this.viewContainer.clear();
    this.viewContainer.createEmbeddedView(this.templateRef);
  }
}
