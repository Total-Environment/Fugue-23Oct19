import React from 'react';
import chai, { expect } from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { App, __RewireAPI__ as AppRewired } from '../src/app';

chai.use(chaiEnzyme());

describe('App', () => {
  describe('render', () => {
    it('should render topbar, sidebar and its children', () => {
      AppRewired.__Rewire__('app', 'app');
      const wrapper = shallow(<App><h1 /></App>);
      expect(wrapper).to.have.className('app');
      expect(wrapper).to.have.descendants('Topbar');
      expect(wrapper).to.have.descendants('Sidebar');
      expect(wrapper).to.have.descendants('h1');
    });

    it('should render sidebar with collapsed as true it has children', () => {
      AppRewired.__Rewire__('app', 'app');
      const wrapper = shallow(<App><h1 /></App>);
      const sidebar = wrapper.find('Sidebar');
      expect(sidebar).to.have.prop('collapsed').to.be.true;
    });

    it('should render sidebar with with collapsed as false when it does not have children', () => {
      AppRewired.__Rewire__('app', 'app');
      const wrapper = shallow(<App></App>);
      const sidebar = wrapper.find('Sidebar');
      expect(sidebar).to.have.prop('collapsed').to.be.false;
    });
  });
});
