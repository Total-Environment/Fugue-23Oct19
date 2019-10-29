import React from 'react';
import {shallow} from 'enzyme';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import sinon from 'sinon';
import {apiUrl, idFor} from '../../src/helpers';

import {Search, __RewireAPI__ as SearchRewired} from '../../src/components/search'

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe("Search", () => {
  let wrapper, props;
  beforeEach(() => {
    props = {
      componentType: 'material',
    };
    wrapper = shallow(<Search {...props}/>);
  });
  it('should have div with className searchBar', () => {
    SearchRewired.__Rewire__('styles', {searchBar: 'searchBar'});
    wrapper = shallow(<Search {...props}/>);
    expect(wrapper).to.have.descendants('div');
    expect(wrapper.find('div').first()).to.have.className('searchBar');
  });
  it('should have form inside the div', () => {
    expect(wrapper).to.have.descendants('div');
    expect(wrapper.find('div').first()).to.have.descendants('form');
  });
  it('should have input text inside the form', () => {
    expect(wrapper.find('div').first().find('form').first()).to.have.descendants('input');
  });
  it('should have Search icon inside div and form tag', () => {
    expect(wrapper.find('div').first().children().find('form').first()).to.have.descendants('Icon');
  });
  it('should render ComponentGroupDropdown when props are present', async() => {
    SearchRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({status: 200, data: ['abc', 'xyz']}))});
    wrapper.find('input').simulate('change', {target: {value: 'keyword'}});
    const preventDefaultSpy = sinon.spy();
    await wrapper.find('form').simulate('submit', {preventDefault: preventDefaultSpy})
    wrapper.find('input').simulate('focus');
    expect(preventDefaultSpy).to.have.been.called;
    expect(wrapper.find('div').first().children()).to.have.descendants('ComponentGroupDropdown');
  });

  it('should push the SFG search URL to browserHistory', () => {
    props.componentType = 'sfg';
    wrapper = shallow(<Search {...props}/>);
    const preventDefaultSpy = sinon.spy();
    const browserHistoryPushSpy = sinon.spy();
    SearchRewired.__Rewire__('browserHistory', {push: browserHistoryPushSpy});
    wrapper.find(`#${idFor('keyword to search')}`).simulate('change', {target: {value: 'SFG0001'}});
    wrapper.find('form').simulate('submit', {preventDefault: preventDefaultSpy});
    expect(preventDefaultSpy).to.have.been.called;
    expect(browserHistoryPushSpy).to.have.been.calledWith('/sfgs/search/SFG0001');
    // expect(wrapper.find('div').first().children()).to.have.not.descendants('ComponentGroupDropdown');
  });

  it('should not shown drop down if component type is sfg',async () => {
    props.componentType = 'sfg';
    wrapper = shallow(<Search {...props}/>);
    SearchRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({status: 200, data: ['abc', 'xyz']}))});
    wrapper.find('input').simulate('change', {target: {value: 'keyword'}});
    const preventDefaultSpy = sinon.spy();
    await wrapper.find('form').simulate('submit', {preventDefault: preventDefaultSpy});
    wrapper.find('input').simulate('focus');
    expect(preventDefaultSpy).to.have.been.called;
    expect(wrapper.find('div').first().children()).to.not.have.descendants('ComponentGroupDropdown');
  });

  it('should not shown drop down if component type is package',async () => {
    props.componentType = 'package';
    wrapper = shallow(<Search {...props}/>);
    SearchRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({status: 200, data: ['abc', 'xyz']}))});
    wrapper.find('input').simulate('change', {target: {value: 'keyword'}});
    const preventDefaultSpy = sinon.spy();
    await wrapper.find('form').simulate('submit', {preventDefault: preventDefaultSpy});
    wrapper.find('input').simulate('focus');
    expect(preventDefaultSpy).to.have.been.called;
    expect(wrapper.find('div').first().children()).to.not.have.descendants('ComponentGroupDropdown');
  });

  it('should push the Package search URL to browserHistory', () => {
    props.componentType = 'package';
    wrapper = shallow(<Search {...props}/>);
    const preventDefaultSpy = sinon.spy();
    const browserHistoryPushSpy = sinon.spy();
    SearchRewired.__Rewire__('browserHistory', {push: browserHistoryPushSpy});
    wrapper.find(`#${idFor('keyword to search')}`).simulate('change', {target: {value: 'PKG0001'}});
    wrapper.find('form').simulate('submit', {preventDefault: preventDefaultSpy});
    expect(preventDefaultSpy).to.have.been.called;
    expect(browserHistoryPushSpy).to.have.been.calledWith('/packages/search/PKG0001')
  });
});
