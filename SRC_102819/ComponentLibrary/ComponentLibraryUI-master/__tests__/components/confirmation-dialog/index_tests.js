import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { ConfirmationDialog, __RewireAPI__ as ConfirmationDialogRewired } from '../../../src/components/confirmation-dialog';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Confirmation Dialog', () => {
  describe('render', () => {
    it('should render React Modal with message', () => {
      ConfirmationDialogRewired.__Rewire__('styles', {dialog: 'confirmation-dialog'});
      const wrapper = shallow(<ConfirmationDialog message="Abdulsattar" title="Heading" shown />);
      expect(wrapper).to.have.className('confirmation-dialog');
      expect(wrapper).to.have.descendants('Modal');
      expect(wrapper.find('Modal')).prop('isOpen').to.be.true;
      expect(wrapper.find('p')).to.have.text('Abdulsattar');
      expect(wrapper.find('h3')).to.have.text('Heading');
    });

    it('should call onYes when Yes is clicked', () => {
      const onYesSpy = sinon.spy();
      const wrapper = shallow(<ConfirmationDialog message="Abdulsattar" shown onYes={onYesSpy} />);
      wrapper.find('button#yes').simulate('click');
      expect(onYesSpy).to.have.been.called;
    });

    it('should call onNo when No is clicked', () => {
      const onNoSpy = sinon.spy();
      const wrapper = shallow(<ConfirmationDialog message="Abdulsattar" shown onNo={onNoSpy} />);
      wrapper.find('button#no').simulate('click');
      expect(onNoSpy).to.have.been.called;
    });
  });
});
