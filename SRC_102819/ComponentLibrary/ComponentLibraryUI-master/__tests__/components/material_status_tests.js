import chai, { expect } from 'chai';
import { shallow } from 'enzyme';
import React from 'react';
import { ComponentStatus,  __RewireAPI__ as ComponentStatusRewired } from '../../src/components/component-status';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('Material Status', () => {
  it('should render header name and column value with proper class name', () => {
    ComponentStatusRewired.__Rewire__('styles', {approved: 'approved'});
    const props = {value: "Approved"};
    const wrapper = shallow(<ComponentStatus {...props}/>);
    expect(wrapper.prop('className')).to.equal('approved');
  });
});
