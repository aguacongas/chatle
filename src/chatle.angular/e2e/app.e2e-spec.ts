import { Chatle.AngularPage } from './app.po';

describe('chatle.angular App', () => {
  let page: Chatle.AngularPage;

  beforeEach(() => {
    page = new Chatle.AngularPage();
  });

  it('should display welcome message', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('Welcome to app!');
  });
});
