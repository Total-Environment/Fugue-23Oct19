import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { InputNumber, __RewireAPI__ as InputNumberRewired } from '../../../src/components/input-number'


chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('InputNumber', () => {
  describe('render', () => {
    let wrapper, props;
    beforeEach(() => {
      props = {
        value: 1,
        className: 'sattar',
        name: 'name',
        onChange: sinon.spy(),
        allowDecimals: true
      };
      wrapper = shallow(<InputNumber {...props}/>);
    });

    it('should render input with passed name', () =>{
      expect(wrapper).to.have.prop('name').to.equal('name');
    });

    it('should render input with the classname provided', () => {
      expect(wrapper).to.have.className(props.className);
    });

    it('should render input with the value provided', () => {
      expect(wrapper).to.have.prop('value').to.equal(props.value);
    });

    it('should render input with empty if value is null', () => {
      props.value = null;
      wrapper = shallow(<InputNumber {...props}/>);
      expect(wrapper).to.have.prop('value').to.equal('');
    });

    it('should call onChange when input value changes', () => {
      wrapper.simulate('change', {target: {value: '1'}});
      expect(props.onChange).to.have.been.calledWith('1');
    });

    it('should call onChange with null when value is empty', () => {
      wrapper.simulate('change', {target: {value: ''}});
      expect(props.onChange).to.have.been.calledWith(null);
    });

    it('should not call onChange with e removed', () => {
      wrapper.simulate('change', {target: {value: '3ee'}});
      expect(props.onChange).to.have.been.calledWith('3');
    });
    it('should not call onChange with null if value is only e', () => {
      wrapper.simulate('change', {target: {value: 'ee'}});
      expect(props.onChange).to.have.been.calledWith(null);
    });
    it('should not call onChange with null if value non numeric', () => {
      wrapper.simulate('change', {target: {value: '+-'}});
      expect(props.onChange).to.have.been.calledWith(null);
    });

    it('should not allow decimal if allowDecimals is false', () => {
      props.allowDecimals = false;
      wrapper = shallow(<InputNumber {...props}/>);
      wrapper.simulate('change', {target: {value: '1.'}});
      expect(props.onChange).to.have.been.calledWith('1');
    });
  });
});
