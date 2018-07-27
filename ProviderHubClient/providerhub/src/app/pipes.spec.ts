import { GenderPipe, NullablePipe, BoolPipe, SpecialtyTypePipe, ParentSpecialtyPipe } from './pipes';

describe('GenderPipe', () => {
  it('create an instance', () => {
    const pipe = new GenderPipe();
    expect(pipe).toBeTruthy();
  });
});
describe('NullablePipe', () => {
  it('create an instance', () => {
    const pipe = new NullablePipe();
    expect(pipe).toBeTruthy();
  });
});
describe('BoolPipe', () => {
  it('create an instance', () => {
    const pipe = new BoolPipe();
    expect(pipe).toBeTruthy();
  });
});
describe('SpecialtyTypePipe', () => {
  it('create an instance', () => {
    const pipe = new SpecialtyTypePipe();
    expect(pipe).toBeTruthy();
  });
});
describe('ParentSpecialtyPipe', () => {
  it('create an instance', () => {
    const pipe = new ParentSpecialtyPipe();
    expect(pipe).toBeTruthy();
  });
});
